using System;
using Reflex.Attributes;

public class Person
{
    public int Age { get; private set; }
    public string Name { get; private set; }
    public DateTime BornAt { get; private set; }

    public Person()
    {
    }

    public Person(int age, string name, DateTime bornAt)
    {
        Age = age;
        Name = name;
        BornAt = bornAt;
    }

    [Inject]
    public void Inject(int age, string name, DateTime bornAt)
    {
        Age = age;
        Name = name;
        BornAt = bornAt;
    }
}