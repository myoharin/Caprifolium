using Microsoft.VisualBasic;
using SineVita.Lonicera;

Func<int, int, int> multiply = (x, y) => x * y;
Func<int, int, Tuple<int,int>> combine = (x, y) => new Tuple<int, int>(x,y);
Console.WriteLine("start");
Lonicera<int, Tuple<int,int>> loniTest = new Lonicera<int, Tuple<int,int>>(combine);
void print(){
    int index = 0;
    Console.WriteLine(loniTest.Links.Count());
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

loniTest.Add(0);
loniTest.Add(1);
loniTest.AddRange(new List<int>(){2,3,4,5,6,7,8,9});

print();
loniTest.Remove(3);
print();


List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

Console.WriteLine("Original List:");
PrintList(numbers);
numbers.RemoveRange(3, 3);
Console.WriteLine("After RemoveRange:");
PrintList(numbers);
