using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace LiteDB.Tests.Database
{
    public class SelectMany_Tests
    {
        #region Model
        
        public class City
        {
            public City(string name) => Name = name;

            public string Name { get; }
        }

        public class Country
        {
            public int Id { get; set; }
            
            public string Name { get; set; }
            
            public City[] Cities { get; set; }
        }

        #endregion

        [Fact]
        public void QueryDocument()
        {
            string json = @"{ Name: 'USA', Cities: [ { Name: 'Denver'}, { Name: 'Washington'} ]}";
            var doc = JsonSerializer.Deserialize(json).AsDocument;
            
            // var doc = new BsonDocument
            // {
            //     ["Name"] = "USA",
            //     ["Cities"] = new BsonArray
            //     {
            //         new BsonDocument
            //         {
            //             ["Name"] = "Denver"
            //         },
            //         new BsonDocument
            //         {
            //             ["Name"] = "Washington"
            //         }
            //     }
            // };

            var expr = BsonExpression.Create("$.Name");
            var expr1 = BsonExpression.Create("$.Cities[*]");
            var expr2 = BsonExpression.Create("$.Cities[*].Name");
            var expr4 = BsonExpression.Create("$");
            var expr5 = BsonExpression.Create("COUNT($.Cities[*])");
            var expr6 = BsonExpression.Create("ARRAY($.Cities[*].Name)");
            var expr7 = BsonExpression.Create("MAP([1,2,3] => [@, @, @])");
            var result1 = expr2.Execute(doc);
            var result2 = expr7.Execute(doc);
        }
        
        [Fact]
        public void SelectMany()
        {
            using (var db = new LiteDatabase(new MemoryStream()))
            {
                var col = db.GetCollection<Country>("Countries");

                col.Insert(new Country { Name = "USA", Cities = new [] { new City("Denver"), new City("Washington")}});
                col.Insert(new Country { Name = "Germany", Cities = new [] { new City("Frankfurt"), new City("Berlin")}});
                col.Insert(new Country { Name = "Marokko", Cities = new [] { new City("Marokko")}});
                
                var result = col.Query().SelectMany("Cities[*]").ToArray();
            }
        }
    }
}