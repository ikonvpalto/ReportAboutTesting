﻿using System.Collections.Generic;

namespace XUnitTestProject
{
    public interface IWarehouse
    {
        bool IsHave(string product, int amount);
        void Take(string product, int amount);
    }

    internal class Warehouse: Dictionary<string, int>, IWarehouse
    {
        public bool IsHave(string product, int amount)
        {
            return ContainsKey(product) && this[product] >= amount;
        }

        public void Take(string product, int amount)
        {
            if (!ContainsKey(product))
            {
                return;
            }

            this[product] -= amount;
        }
    }

    internal class Order
    {
        public string Product { get; set; }
        public int Amount { get; set; }
        private bool _isFilled { get; set; } = false;
        public bool IsFilled => _isFilled;

        public void Fill(IWarehouse warehouse)
        {
            if (_isFilled)
            {
                return;
            }

            if (!warehouse.IsHave(Product, Amount))
            {
                return;
            }

            _isFilled = true;
            warehouse.Take(Product, Amount);
        }
    }
}
