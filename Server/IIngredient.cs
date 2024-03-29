using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server
{
    public interface IIngredient
    {
        void Accept(IVisitor visitor);
        string Name { get; }
        bool IsGlutenFree { get; }
        bool IsVegetarian { get; }
        bool IsLactoseFree { get; }
    }


    //Add allll the things you want... Seems like a lot of the same code over and over again but not sure how to improve this

    public class PizzaBottom : IIngredient
    {
        public string Name { get; }
        public bool IsGlutenFree { get; }
        public bool IsVegetarian { get; }
        public bool IsLactoseFree { get; }

        public PizzaBottom(string name, bool isGlutenFree, bool isVegetarian, bool isLactoseFree)
        {
            Name = name;
            IsGlutenFree = isGlutenFree;
            IsVegetarian = isVegetarian;
            IsLactoseFree = isLactoseFree;
        }
        public void Accept(IVisitor visitor)
        {
            visitor.Visit();
        }
    }

    public class PizzaSauce : IIngredient
    {
        public string Name { get; }
        public bool IsGlutenFree { get; }
        public bool IsVegetarian { get; }
        public bool IsLactoseFree { get; }

        public PizzaSauce(string name, bool isGlutenFree, bool isVegetarian, bool isLactoseFree)
        {
            Name = name;
            IsGlutenFree = isGlutenFree;
            IsVegetarian = isVegetarian;
            IsLactoseFree = isLactoseFree;
        }
        public void Accept(IVisitor visitor)
        {
            visitor.Visit();
        }
    }

    public class Meat : IIngredient
    {
        public string Name { get; }
        public bool IsGlutenFree { get; }
        public bool IsVegetarian { get; }
        public bool IsLactoseFree { get; }

        public Meat(string name, bool isGlutenFree, bool isVegetarian, bool isLactoseFree)
        {
            Name = name;
            IsGlutenFree = isGlutenFree;
            IsVegetarian = isVegetarian;
            IsLactoseFree = isLactoseFree;
        }
        public void Accept(IVisitor visitor)
        {
            visitor.Visit();
        }
    }

    public class Dairy : IIngredient
    {
        public string Name { get; }
        public bool IsGlutenFree { get; }
        public bool IsVegetarian { get; }
        public bool IsLactoseFree { get; }

        public Dairy(string name, bool isGlutenFree, bool isVegetarian, bool isLactoseFree)
        {
            Name = name;
            IsGlutenFree = isGlutenFree;
            IsVegetarian = isVegetarian;
            IsLactoseFree = isLactoseFree;
        }
        public void Accept(IVisitor visitor)
        {
            visitor.Visit();
        }
    }

    public class Vegetable : IIngredient
    {
        public string Name { get; }
        public bool IsGlutenFree { get; }
        public bool IsVegetarian { get; }
        public bool IsLactoseFree { get; }

        public Vegetable(string name, bool isGlutenFree, bool isVegetarian, bool isLactoseFree)
        {
            Name = name;
            IsGlutenFree = isGlutenFree;
            IsVegetarian = isVegetarian;
            IsLactoseFree = isLactoseFree;
        }
        public void Accept(IVisitor visitor)
        {
            visitor.Visit();
        }
    }
}
