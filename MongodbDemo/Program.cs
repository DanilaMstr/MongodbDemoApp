using System;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace MongodbDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            MongoCRUD db = new MongoCRUD("AddressBook");

            //PersonModel person = new PersonModel
            //{
            //    FirstName = "Danila",
            //    LastName = "Ponukaev",
            //    PrimaryAddress = new AddressModel
            //    {
            //        StreetAdress = "11-ay Parcovay 36-1116",
            //        City = "Moscow",
            //        State = "RUS",
            //        ZipCode = "105077"
            //    }
            //};

            //db.InsertRecord("Users", person);

            //foreach (var rec in db.LoadRecords<PersonModel>("Users"))
            //{
            //    Console.WriteLine(rec);
            //}

            foreach (var rec in db.LoadRecords<NameModel>("Users"))
            {
                Console.WriteLine($"{rec.Id}: {rec.FirstName} {rec.LastName}");

                Console.WriteLine();
            }

            //var oneRec = db.LoadRecordById<PersonModel>("Users", new Guid("bdbb487f-f04e-4e5d-ab14-cd174cea376c"));
            //Console.WriteLine(oneRec);

            //oneRec.DateOfBirth = new DateTime(1999, 12, 6, 0, 0, 0, DateTimeKind.Utc);
            //db.UpsertRecord("Users", oneRec.Id, oneRec);
            //Console.WriteLine(oneRec);

            //db.DeleteRecord<PersonModel>("Users", oneRec.Id);
        }
    }

    public class NameModel
    {
        [BsonId] // _id
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class PersonModel
    {
        [BsonId] // _id
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public AddressModel PrimaryAddress { get; set; }
        public DateTime DateOfBirth { get; set; } 

        public override string ToString()
        {
            string res = $"{Id}\n" +
                $"{FirstName} {LastName}\n";

            if (PrimaryAddress != null)
                res += PrimaryAddress;

            if (DateOfBirth != null)
                res += DateOfBirth.ToString() + "\n";

            res += "----------------------------------------";

            return res;
        }
    }

    public class AddressModel
    {
        public string StreetAdress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        public override string ToString() =>
            $"{StreetAdress} | {City} | {State} | {ZipCode}\n";
    }

    public class MongoCRUD
    {
        private IMongoDatabase db;

        public MongoCRUD(string database)
        {
            var client = new MongoClient();
            db = client.GetDatabase(database);
        }

        public void InsertRecord<T>(string table, T record)
        {
            var collection = db.GetCollection<T>(table);
            collection.InsertOne(record);
        }

        public List<T> LoadRecords<T>(string table)
        {
            var collection = db.GetCollection<T>(table);

            return collection.Find(new BsonDocument()).ToList();
        }

        public T LoadRecordById<T>(string table, Guid id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);

            return collection.Find(filter).First();
        }

        public void UpsertRecord<T>(string table, Guid id, T record)
        {
            var collection = db.GetCollection<T>(table);

            var result = collection.ReplaceOne(
                new BsonDocument("_id", id),
                record,
                new UpdateOptions { IsUpsert = true });
        }

        public void DeleteRecord<T>(string table, Guid id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);
            collection.DeleteOne(filter);
        }
    }
}
