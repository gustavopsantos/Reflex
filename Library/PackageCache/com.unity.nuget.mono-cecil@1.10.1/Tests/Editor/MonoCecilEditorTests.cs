using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Mono.Cecil;
using Mono.Cecil.Cil;

class MonoCecilEditorTests {

	[Test]
	public void EditorSampleTestSimplePasses() {
		Assert.IsTrue(true);
	}
}
