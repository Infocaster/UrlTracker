import { ICustomEditor } from "@/umbraco/editor.service";

export interface IScope extends angular.IScope {
  $parent: IParentScope;
  model: ICustomEditor;
}

interface IParentScope extends angular.IScope {
  //   model: {
  //     config: {
  //       projectCode: string;
  //       projectName: string;
  //     };
  //   };
}
