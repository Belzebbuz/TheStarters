import { AssignRole } from './user.contracts';

export interface IUser {
  id: string;
  email: string;
  roles: AssignRole[];
  isActive: boolean;
}

export interface ITokenUser {
  id: string;
  email: string;
  roles: string[];
}
