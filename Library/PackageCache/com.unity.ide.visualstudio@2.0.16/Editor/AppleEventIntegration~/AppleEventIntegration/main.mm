/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Unity Technologies.
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
 
#import <Cocoa/Cocoa.h>
#import <Foundation/Foundation.h>

// 'FSnd' FourCC
#define keyFileSender                                   1179872868

// 16 bit aligned legacy struct - this should total 20 bytes
struct SelectionRange
{
    int16_t unused1;    // 0 (not used)
    int16_t lineNum;    // line to select (<0 to specify range)
    int32_t startRange; // start of selection range (if line < 0)
    int32_t endRange;   // end of selection range (if line < 0)
    int32_t unused2;    // 0 (not used)
    int32_t theDate;    // modification date/time
} __attribute__((packed));

static NSString* MakeNSString(const char* str)
{
    if (!str)
        return NULL;

    NSString* ret = [NSString stringWithUTF8String: str];
    return ret;
}

static UInt32 GetCreatorOfThisApp()
{
    static UInt32 creator = 0;
    if (creator == 0)
    {
        UInt32 type;
        CFBundleGetPackageInfo(CFBundleGetMainBundle(), &type, &creator);
    }
    return creator;
}

static BOOL OpenFileAtLineWithAppleEvent(NSRunningApplication *runningApp, NSString* path, int line)
{
    if (!runningApp)
        return NO;

    NSURL *pathUrl = [NSURL fileURLWithPath: path];
    
    NSAppleEventDescriptor* targetDescriptor = [NSAppleEventDescriptor
        descriptorWithProcessIdentifier: runningApp.processIdentifier];

    NSAppleEventDescriptor* appleEvent = [NSAppleEventDescriptor
		appleEventWithEventClass: kCoreEventClass
		eventID: kAEOpenDocuments
		targetDescriptor: targetDescriptor
		returnID: kAutoGenerateReturnID
		transactionID: kAnyTransactionID];

    [appleEvent
		setParamDescriptor: [NSAppleEventDescriptor
			descriptorWithDescriptorType: typeFileURL
			data: [[pathUrl absoluteString] dataUsingEncoding: NSUTF8StringEncoding]]
		forKeyword: keyDirectObject];

    UInt32 packageCreator = GetCreatorOfThisApp();
    if (packageCreator == kUnknownType) {
        [appleEvent
			setParamDescriptor: [NSAppleEventDescriptor
				descriptorWithDescriptorType: typeApplicationBundleID
				data: [[[NSBundle mainBundle] bundleIdentifier] dataUsingEncoding: NSUTF8StringEncoding]]
			forKeyword: keyFileSender];
    } else {
        [appleEvent
			setParamDescriptor: [NSAppleEventDescriptor descriptorWithTypeCode: packageCreator]
			forKeyword: keyFileSender];
    }
    
    if (line != -1) {
        // Add selection range to event
        SelectionRange range;
        range.unused1 = 0;
        range.lineNum = line - 1;
        range.startRange = -1;
        range.endRange = -1;
        range.unused2 = 0;
        range.theDate = -1;
        
        [appleEvent
			setParamDescriptor: [NSAppleEventDescriptor
				descriptorWithDescriptorType: typeChar
				bytes: &range
				length: sizeof(SelectionRange)]
			forKeyword: keyAEPosition];
    }

    AEDesc reply = { typeNull, NULL };
    OSErr err = AESendMessage(
		[appleEvent aeDesc],
		&reply,
		kAENoReply + kAENeverInteract,
		kAEDefaultTimeout);

    return err == noErr;
}

static BOOL ApplicationSupportsQueryOpenedSolution(NSString* appPath)
{
    NSURL* appUrl = [NSURL fileURLWithPath: appPath];
    NSBundle* bundle = [NSBundle bundleWithURL: appUrl];

    if (!bundle)
        return NO;

    id versionValue = [bundle objectForInfoDictionaryKey: @"CFBundleVersion"];
    if (!versionValue || ![versionValue isKindOfClass: [NSString class]])
        return NO;

    NSString* version = (NSString*)versionValue;
    return [version compare:@"8.6" options:NSNumericSearch] != NSOrderedAscending;
}

static NSArray<NSRunningApplication*>* QueryRunningInstances(NSString *appPath)
{
    NSMutableArray<NSRunningApplication*>* instances = [[NSMutableArray alloc] init];
    NSURL *appUrl = [NSURL fileURLWithPath: appPath];

    for (NSRunningApplication *runningApp in NSWorkspace.sharedWorkspace.runningApplications) {
        if (![runningApp isTerminated] && [runningApp.bundleURL isEqual: appUrl]) {
            [instances addObject: runningApp];
        }
    }

    return instances;
}

enum {
    kWorkspaceEventClass = 1448302419, /* 'VSWS' FourCC */
    kCurrentSelectedSolutionPathEventID = 1129534288 /* 'CSSP' FourCC */
};

static BOOL TryQueryCurrentSolutionPath(NSRunningApplication* runningApp, NSString** solutionPath)
{
    NSAppleEventDescriptor* targetDescriptor = [NSAppleEventDescriptor
        descriptorWithProcessIdentifier: runningApp.processIdentifier];

    NSAppleEventDescriptor* appleEvent = [NSAppleEventDescriptor
        appleEventWithEventClass: kWorkspaceEventClass
        eventID: kCurrentSelectedSolutionPathEventID
        targetDescriptor: targetDescriptor
        returnID: kAutoGenerateReturnID
        transactionID: kAnyTransactionID];

    AEDesc aeReply = { 0, };

    OSErr sendResult = AESendMessage(
        [appleEvent aeDesc],
        &aeReply,
        kAEWaitReply | kAENeverInteract,
        kAEDefaultTimeout);

    if (sendResult != noErr) {
        return NO;
    }

    NSAppleEventDescriptor *reply = [[NSAppleEventDescriptor alloc] initWithAEDescNoCopy: &aeReply];
    *solutionPath = [[reply descriptorForKeyword: keyDirectObject] stringValue];

    return *solutionPath != NULL;
}

static NSRunningApplication* QueryRunningApplicationOpenedOnSolution(NSString* appPath, NSString* solutionPath)
{
    BOOL supportsQueryOpenedSolution = ApplicationSupportsQueryOpenedSolution(appPath);

    for (NSRunningApplication *runningApp in QueryRunningInstances(appPath)) {
        // If the currently selected external editor does not support the opened solution apple event
        // then fallback to the previous behavior: take the first opened VSM and open the solution
        if (!supportsQueryOpenedSolution) {
            OpenFileAtLineWithAppleEvent(runningApp, solutionPath, -1);
            return runningApp;
        }

        NSString* currentSolutionPath;
        if (TryQueryCurrentSolutionPath(runningApp, &currentSolutionPath)) {
            if ([solutionPath isEqual:currentSolutionPath]) {
    	        return runningApp;
            }
        } else {
            // If VSM doesn't respond to the query opened solution event
            // we fallback to the previous behavior too
            OpenFileAtLineWithAppleEvent(runningApp, solutionPath, -1);
            return runningApp;
        }
    }
    
    return NULL;
}

static NSRunningApplication* LaunchApplicationOnSolution(NSString* appPath, NSString* solutionPath)
{
    NSURL* appUrl = [NSURL fileURLWithPath: appPath];
    NSMutableDictionary* config = [[NSMutableDictionary alloc] init];

    NSRunningApplication* runningApp = [[NSWorkspace sharedWorkspace]
        launchApplicationAtURL: appUrl
        options: NSWorkspaceLaunchDefault | NSWorkspaceLaunchNewInstance
        configuration: config
        error: nil];

    OpenFileAtLineWithAppleEvent(runningApp, solutionPath, -1);
    
    return runningApp;
}

static NSRunningApplication* QueryOrLaunchApplication(NSString* appPath, NSString* solutionPath)
{
    NSRunningApplication* runningApp = QueryRunningApplicationOpenedOnSolution(appPath, solutionPath);
    
    if (!runningApp)
        runningApp = LaunchApplicationOnSolution(appPath, solutionPath);

    if (runningApp)
        [runningApp activateWithOptions: 0];
    
    return runningApp;
}

BOOL LaunchOrReuseApp(NSString* appPath, NSString* solutionPath, NSRunningApplication** outApp)
{
    NSRunningApplication* app = QueryOrLaunchApplication(appPath, solutionPath);
    
    if (outApp)
        *outApp = app;

    return app != NULL;
}

BOOL MonoDevelopOpenFile(NSString* appPath, NSString* solutionPath, NSString* filePath, int line)
{
    NSRunningApplication* runningApp;
    if (!LaunchOrReuseApp(appPath, solutionPath, &runningApp)) {
        return FALSE;
    }

    if (filePath) {
        return OpenFileAtLineWithAppleEvent(runningApp, filePath, line);
    }

    return YES;
}

#if BUILD_APP

int main(int argc, const char** argv)
{
    if (argc != 5) {
        printf("Usage: AppleEventIntegration appPath solutionPath filePath lineNumber\n");
        return 1;
    }

    const char* appPath = argv[1];
    const char* solutionPath = argv[2];
    const char* filePath = argv[3];
    const int lineNumber = atoi(argv[4]);

    @autoreleasepool
    {
        MonoDevelopOpenFile(MakeNSString(appPath), MakeNSString(solutionPath), MakeNSString(filePath), lineNumber);
    }

    return 0;
}

#else

extern "C"
{
	BOOL OpenVisualStudio(const char* appPath, const char* solutionPath, const char* filePath, int line)
	{
    	return MonoDevelopOpenFile(MakeNSString(appPath), MakeNSString(solutionPath), MakeNSString(filePath), line);
	}
}

#endif
