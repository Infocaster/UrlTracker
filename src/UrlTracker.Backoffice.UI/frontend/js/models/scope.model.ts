import { ICustomEditor } from '@/umbraco/editor.service';

export interface IScope {
  $parent: IParentScope;
  model: ICustomEditor;
  [key: string]: any;
}

interface IParentScope {
  model: {
    config: {
      projectCode: string;
      projectName: string;
    };
  };

  [key: string]: any;
}
