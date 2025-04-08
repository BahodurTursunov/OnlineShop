import {BaseModel} from './baseModel';
import {IProduct} from '../interfaces/product';

export class Product extends BaseModel implements IProduct {
  name: string;
  description: string;
  price: number;
  stock: number;
  imageUrl: string;
  categoryId: number;
  discount: number;
  createdAt: Date;

  constructor(id: number, name: string, description: string, price: number, stock: number, imageUrl: string, categoryId: number, discount: number) {
    super(id);
    this.name = name;
    this.description = description;
    this.price = price;
    this.stock = stock;
    this.imageUrl = imageUrl;
    this.categoryId = categoryId;
    this.discount = discount;
    this.createdAt = new Date();

  }
}
