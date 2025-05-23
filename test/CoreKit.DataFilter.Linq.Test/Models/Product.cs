﻿namespace CoreKit.DataFilter.Linq.Tests.Models;

public class Product
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
}