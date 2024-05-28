import { Axios, AxiosResponse } from "axios";
import { axiosInstance } from "../util/tools/axios.service";
import urlresource, {
  IControllerUrlResource,
  IUrlResource,
} from "../util/tools/urlresource.service";
import { IRedirectResponse } from "./redirect.service";

export interface IRedirectImportService {
  export: () => Promise<Blob>;
  exportTemplate: () => Promise<Blob>;
  import: (request: File) => Promise<IRedirectResponse[]>;
}

export class RedirectImportService implements IRedirectImportService {
  constructor(private axios: Axios, private urlResource: IUrlResource) {}

  private get controller(): IControllerUrlResource {
    return this.urlResource.getController("RedirectImport");
  }

  public async exportTemplate() {
    let response = await this.axios.get<Blob>(this.controller.getUrl("ExportExample"));
    this.downloadBlob(response, 'redirect-template');
    return response.data;
  }

  public async export() {
    let response = await this.axios.get<Blob>(this.controller.getUrl("Export"));
    this.downloadBlob(response, 'redirects');
    return response.data;
  }

  private downloadBlob(response: AxiosResponse<Blob, any>, fileName: string): void {

    const blob = new Blob([response.data], {type: 'text/csv;charset=utf-8;'});
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', fileName + '.csv');
    document.body.appendChild(link);
    link.click();
  }

  public async import(file: File) {
    const form = new FormData();
    form.append("Redirects", file, file.name);
    let response = await this.axios.post<IRedirectResponse[]>(
      this.controller.getUrl("Import"),
      form
    );

    return response.data;
  }
}

export default new RedirectImportService(axiosInstance, urlresource);
