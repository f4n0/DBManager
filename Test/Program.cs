// See https://aka.ms/new-console-template for more information
using DBManager.Infrastructure;
using Test.Models;
using DBManager;

Console.WriteLine("Hello, World!");

//initialize the DBManager (could be in startup.cs)
//DBManager.DBManager.Instance.UseLiteDB("C:\\_myTmp\\db.litedb");
DBManager.DBManager.Instance.UseSQLite("C:\\_myTmp\\db.sqlite");

// To Create a new item --> RecordFactory.CreateModel<T>();
// To Read an Item --> RecordFactory.Open<T>().Read();

// To Insert/Delete/Update T.Store/T.Delete

//var parent = RecordFactory.CreateModel<Parent>();
//parent.TestString = "test";
//parent.TestInt = 1;
//parent.TestDateTime = DateTime.Now;
//parent.TestEnum = myEnum.Second;
//parent.Store();

//parent = RecordFactory.CreateModel<Parent>();
//parent.TestString = "Test2";
//parent.TestInt = 2;
//parent.TestDateTime = DateTime.Now;
//parent.TestEnum = myEnum.Second;
//parent.Store();

//var Child = RecordFactory.CreateModel<Child>();
//Child.Data1 = "Test Data";
//Child.ParentId = parent.Id;
//Child.Store();


var read = RecordFactory.Open<Parent>().Read();


read = RecordFactory.Open<Parent>().SetRange(parent => parent.TestString == "Test2").Read();



Console.ReadKey();