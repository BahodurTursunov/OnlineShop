﻿using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Entities
{
    public class CartItem /*данные корзины*/ : BaseEntity
    {
        public decimal Amount { get; set; }

        public int CartId { get; set; }
        public Cart? Cart { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Количество товара должно быть больше 0")]
        public int Quantity { get; set; } // количество
    }
}