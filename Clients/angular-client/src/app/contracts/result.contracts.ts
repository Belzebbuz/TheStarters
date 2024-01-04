export interface IResult {
  succeeded: boolean;
  messages: string[];
}

export interface IDataResult<T> extends IResult {
  data: T;
}

export interface IPagedResult<T> extends IResult {
  data: T[];
  currentPage: number;
  itemsPerPage: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
