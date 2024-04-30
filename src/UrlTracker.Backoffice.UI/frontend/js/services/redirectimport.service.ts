import { Axios } from "axios";
import { axiosInstance } from "../util/tools/axios.service";
import urlresource, {
  IControllerUrlResource,
  IUrlResource,
} from "../util/tools/urlresource.service";
import { IRedirectResponse } from "./redirect.service";

export interface IRedirectImportService {
  export: () => Promise<Blob>;
  import: (request: File) => Promise<IRedirectResponse[]>;
}

export class RedirectImportService implements IRedirectImportService {
  constructor(private axios: Axios, private urlResource: IUrlResource) {}

  private get controller(): IControllerUrlResource {
    return this.urlResource.getController("redirectimport");
  }

  public async export() {
    let response = await this.axios.get<Blob>(this.controller.getUrl("export"));

    return response.data;
  }

  public async import(file: File) {
    const form = new FormData();
    form.append("Redirects", file, file.name);
    let response = await this.axios.post<IRedirectResponse[]>(
      this.controller.getUrl("import"),
      form
    );

    return response.data;
  }
}

export default new RedirectImportService(axiosInstance, urlresource);
