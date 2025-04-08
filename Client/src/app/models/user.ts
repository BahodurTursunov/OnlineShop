import {BaseModel} from './baseModel';
import {IUser} from '../interfaces/user';

export class User extends BaseModel implements IUser {
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  passwordHash: string;
  token: string;

  constructor(id: number, username: string, email: string, firstName: string, lastName: string, passwordHash: string, token: string) {
    super(id);
    this.username = username;
    this.email = email;
    this.firstName = firstName;
    this.lastName = lastName;
    this.passwordHash = passwordHash;
    this.token = token;
  }
}
