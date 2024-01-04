export class PagedUsersQuery {
  constructor(
    public page = 1,
    public itemsPerPage = 10,
    public searchValue?: string
  ) {}
}

export class ToggleUserIsActiveRequest {
  constructor(public userId: string, public isActive: boolean) {}
}
export class AssignRolesRequest {
  constructor(public userId: string, public roles: AssignRole[]) {}
}
export class AssignRole {
  constructor(public isActive: boolean, public role: string) {}
}
