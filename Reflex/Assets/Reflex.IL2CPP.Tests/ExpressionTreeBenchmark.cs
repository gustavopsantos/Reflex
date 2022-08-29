using System;
using UnityEngine;
using Domain.Generics;
using System.Reflection;
using System.Diagnostics;
using System.Linq.Expressions;

public class ExpressionTreeBenchmark : MonoBehaviour
{
	private const int Iterations = 10000;
	private const int SampleCount = 64;
	private readonly Stopwatch _stopwatch = new Stopwatch();

	private readonly RingBuffer<long> _normalGetterBuffer = new RingBuffer<long>(SampleCount);
	private readonly RingBuffer<long> _normalSetterBuffer = new RingBuffer<long>(SampleCount);
	private readonly RingBuffer<long> _reflectionGetterBuffer = new RingBuffer<long>(SampleCount);
	private readonly RingBuffer<long> _reflectionSetterBuffer = new RingBuffer<long>(SampleCount);
	private readonly RingBuffer<long> _expressionGetterBuffer = new RingBuffer<long>(SampleCount);
	private readonly RingBuffer<long> _expressionSetterBuffer = new RingBuffer<long>(SampleCount);

	private FieldInfo _fieldInfo;
	private Func<object, object> _fieldGetter;
	private Action<object, object> _fieldSetter;

	private int _number;
	private GUIStyle _guiStyle;

	private void Start()
	{
		_guiStyle = new GUIStyle("label")
		{
			fontSize = 64,
			alignment = TextAnchor.MiddleCenter
		};
		var type = GetType();
		_fieldInfo = type.GetField("_number", BindingFlags.Instance | BindingFlags.NonPublic);
		_fieldGetter = CompileFieldGetter(type, _fieldInfo);
		_fieldSetter = CompileFieldSetter(type, _fieldInfo);
	}

	private void OnGUI()
	{
		BenchmarkNormalGetter();
		BenchmarkNormalSetter();
		BenchmarkReflectionGetter();
		BenchmarkReflectionSetter();
		BenchmarkExpressionGetter();
		BenchmarkExpressionSetter();

		var cellHeight = (float) Screen.height / 6;
		GUILabel(new Rect(default, 0 * cellHeight, Screen.width, cellHeight), $"Normal Getter: {Average(_normalGetterBuffer)}");
		GUILabel(new Rect(default, 1 * cellHeight, Screen.width, cellHeight), $"Normal Setter: {Average(_normalSetterBuffer)}");
		GUILabel(new Rect(default, 2 * cellHeight, Screen.width, cellHeight), $"Reflection Getter: {Average(_reflectionGetterBuffer)}");
		GUILabel(new Rect(default, 3 * cellHeight, Screen.width, cellHeight), $"Reflection Setter: {Average(_reflectionSetterBuffer)}");
		GUILabel(new Rect(default, 4 * cellHeight, Screen.width, cellHeight), $"Expression Getter: {Average(_expressionGetterBuffer)}");
		GUILabel(new Rect(default, 5 * cellHeight, Screen.width, cellHeight), $"Expression Setter: {Average(_expressionSetterBuffer)}");
	}

	private void GUILabel(Rect area, string content)
	{
		GUI.Label(area, content, _guiStyle);
	}

	private void BenchmarkNormalGetter()
	{
		_stopwatch.Restart();
		for (int i = 0; i < Iterations; i++)
		{
			int number = _number;
		}

		_stopwatch.Stop();
		_normalGetterBuffer.Push(_stopwatch.ElapsedTicks);
	}

	private void BenchmarkNormalSetter()
	{
		_stopwatch.Restart();
		for (int i = 0; i < Iterations; i++)
		{
			_number = 42;
		}

		_stopwatch.Stop();
		_normalSetterBuffer.Push(_stopwatch.ElapsedTicks);
	}

	private void BenchmarkReflectionGetter()
	{
		_stopwatch.Restart();
		for (int i = 0; i < Iterations; i++)
		{
			int number = (int) _fieldInfo.GetValue(this);
		}

		_stopwatch.Stop();
		_reflectionGetterBuffer.Push(_stopwatch.ElapsedTicks);
	}

	private void BenchmarkReflectionSetter()
	{
		_stopwatch.Restart();
		for (int i = 0; i < Iterations; i++)
		{
			_fieldInfo.SetValue(this, 42);
		}

		_stopwatch.Stop();
		_reflectionSetterBuffer.Push(_stopwatch.ElapsedTicks);
	}

	private void BenchmarkExpressionGetter()
	{
		_stopwatch.Restart();
		for (int i = 0; i < Iterations; i++)
		{
			int number = (int) _fieldGetter(this);
		}

		_stopwatch.Stop();
		_expressionGetterBuffer.Push(_stopwatch.ElapsedTicks);
	}

	private void BenchmarkExpressionSetter()
	{
		_stopwatch.Restart();
		for (int i = 0; i < Iterations; i++)
		{
			_fieldSetter(this, 123);
		}

		_stopwatch.Stop();
		_expressionSetterBuffer.Push(_stopwatch.ElapsedTicks);
	}

	private static Func<object, object> CompileFieldGetter(Type type, FieldInfo fieldInfo)
	{
		var ownerParameter = Expression.Parameter(typeof(object));

		var fieldExpression = Expression.Field(Expression.Convert(ownerParameter, type), fieldInfo);

		return Expression.Lambda<Func<object, object>>(Expression.Convert(fieldExpression, typeof(object)), ownerParameter).Compile();
	}

	private static Action<object, object> CompileFieldSetter(Type type, FieldInfo fieldInfo)
	{
		var ownerParameter = Expression.Parameter(typeof(object));
		var fieldParameter = Expression.Parameter(typeof(object));

		var fieldExpression = Expression.Field(Expression.Convert(ownerParameter, type), fieldInfo);

		return Expression.Lambda<Action<object, object>>(
				Expression.Assign(fieldExpression, Expression.Convert(fieldParameter, fieldInfo.FieldType)), ownerParameter, fieldParameter)
			.Compile();
	}

	private static long Average(RingBuffer<long> buffer)
	{
		long total = 0;

		for (int i = 0; i < buffer.Length; i++)
		{
			total += buffer[i];
		}

		return (total / buffer.Length) / Iterations;
	}
}