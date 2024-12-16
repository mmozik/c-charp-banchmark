using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System;

//Transorms csv input into md table. See tests project for sample input and output.
public class Solution {
    record Order(string Product, DateTime Date, float Price);
    static readonly CultureInfo cultureInfo = new CultureInfo("en-US");
    public static void GenerateCrossTable(TextReader reader, TextWriter writer) {
        //Read lines with comma-separated values into a list of orders
        //string input = reader.ReadToEnd();
        //string[] lines = input.Split(Environment.NewLine);
        List<Order> orders = new();
        //for(int i = 0; i < lines.Count(); i++) {
        //    string[] pieces = lines[i].Split(",");
        //    string name = pieces[0];
        //    DateTime date = DateTime.Parse(pieces[1], cultureInfo);
        //    float price = float.Parse(pieces[2], cultureInfo);
        //    orders.Add(new Order(name, date, price));
        //}
        while (reader.Peek() >= 0)
        {
            string line = reader.ReadLine();
            string[] pieces = line.Split(",");
            string name = pieces[0];
            DateTime date = DateTime.Parse(pieces[1], cultureInfo);
            float price = float.Parse(pieces[2], cultureInfo);
            orders.Add(new Order(name, date, price));
        }

        //Calculate cross-table summary values
        Dictionary<Tuple<string, int>, float> sums = new();
        foreach(Order order in orders) {
            var sumKey = Tuple.Create(order.Product, order.Date.Year);
            if(sums.ContainsKey(sumKey))
                sums[sumKey] += order.Price;
            else
                sums[sumKey] = order.Price;
        }

        //Calculate ordered lists of years and product names
        //List<int> years = new();
        //List<string> products = new();
        SortedSet<int> years = new();
        SortedSet<string> products = new();

        foreach(var key in sums.Keys) {
            //if(!years.Contains(key.Item2))
                years.Add(key.Item2);
            //if(!products.Contains(key.Item1))
                products.Add(key.Item1);
        }
        //products.Sort();
        //years.Sort();

        //Produce resulting table
        string header = GenerateTableLine(null, years);
        string delimeter = GenerateTableLine("-", Enumerable.Repeat('-', years.Count));

        writer.Write(header);
        writer.Write(delimeter);
        //List<string> rows = new();
        //StringWriter sw = new StringWriter();
        foreach(var product in products) {
            object[] cells = years.Select(year => {
                var tmp = sums.GetValueOrDefault(Tuple.Create(product, year), 0);
                if (tmp > 0)
                    return tmp.ToString("c", cultureInfo);
                else
                    return string.Empty;
                }).ToArray();
            string row = GenerateTableLine(product, cells);
            writer.Write(row);
            //rows.Add(row);
        }
        //writer.Write(header + delimeter + string.Concat(rows));
    }
    static string GenerateTableLine<T>(object left, IEnumerable<T> values) { 
        return $"|{left}|{string.Join("|", values)}|{Environment.NewLine}";
    }
}