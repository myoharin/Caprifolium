﻿using Caprifolium;
using System;
using System.Collections.Generic;

void TestCode1() {
    Func<int, int, int> multiply = (x, y) => x * y;
    Func<int, int, Tuple<int,int>> combine = (x, y) => new Tuple<int, int>(x,y);
    Func<int, int> factorial = (n) => {
        if (n == 0) return 1;
        int result = 1;
        for (int i = 1; i <= n; i++) {
            result *= i;
        }
        return result;
    };
    Func<int, int, int> combination = (r, n) => {
        if (r > n) return 0;
        return factorial(n) / (factorial(r) * factorial(n - r));
    };
    
    Console.WriteLine("start");
    Lonicera<int, int> loniTest = new Lonicera<int, int>(combination);
    void print(){
        int index = 0;
        Console.WriteLine(loniTest.Links.Count);
        for (int i = 1; i < loniTest.NodeCount; i++) {
            for (int j = 0; j < i; j++) {
                Console.Write($"| {loniTest.Links[index]} ");
                index++;
            }
            Console.Write("\n");
        }
        Console.WriteLine();
        foreach(var link in loniTest.Links) {
            Console.Write($"| {link} ");
        }
        Console.WriteLine();
    }
    void PrintList(List<int> list){
    foreach (int item in list) {
        Console.Write(item + ", ");
    }
    Console.WriteLine();
    }


    for (int i = 1; i< 10; i++) {
    Console.WriteLine($"({i}, {Lonicera<int, int>.NodesToLinkIndex(0,i)})");
    }

    loniTest.AddRange(new List<int>(){0,1,2,3,4,5,6,7,8,9});

    print();
    loniTest.Remove(3);
    print();
    loniTest.Insert(3,3);
    print();
    loniTest.Insert(0,3);


    List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

    Console.WriteLine("Original List:");
    PrintList(numbers);
    numbers.RemoveRange(3, 3);
    Console.WriteLine("After RemoveRange:");
    PrintList(numbers);
    print();
}

void TestCode2() {
    var list = Knautia<int, int>.GetExhaustiveIndices(5,8, 3);
    var i = 0;
    foreach (var line in list) {
        var str = "";
        foreach (var item in line) {
            str += $"{item} ";
        }
        str = $"{i} | [{str}]";
        Console.Out.WriteLine(str);
        i++;
    }
}

void TestCode3() {

    string knautiaGrowth(int[] input) {
        var str = "";
        foreach (int i in input) {
            str = $"{str} {i}";
        }
        return str;
    }

    var knautia = new Knautia<int, string>(4, knautiaGrowth);

    for (int i = 0; i < 9; i++) {
        knautia.Add(i, true);
        Console.WriteLine($"Added {i}");
        int a = 0;
        foreach (string? str in knautia.Links) {
            Console.WriteLine($"{a} | {str ?? "null"}");
            a++;
        }
    }
}


TestCode3();