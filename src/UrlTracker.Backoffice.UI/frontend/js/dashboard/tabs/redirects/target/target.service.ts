import { Axios } from 'axios';
import { axiosInstance } from '../../../../util/tools/axios.service';
import urlResource, { IControllerUrlResource, IUrlResource } from '../../../../util/tools/urlresource.service';

export interface IContentTargetResponse {
  icon: string;
  iconColor?: string;
  name: string;
  url?: string;
}

export interface IContentTargetRequest {
  id: number;
  culture?: string;
}

export interface ITargetService {
  Content: (request: IContentTargetRequest) => Promise<IContentTargetResponse>;
}

export class TargetService implements ITargetService {
  constructor(
    private axios: Axios,
    private urlResource: IUrlResource,
  ) {}

  private get controller(): IControllerUrlResource {
    return this.urlResource.getController('RedirectTarget');
  }

  public async Content(request: IContentTargetRequest): Promise<IContentTargetResponse> {
    const response = await this.axios.get<IContentTargetResponse>(this.controller.getUrl('Content'), {
      params: request,
    });

    if (response.status !== 200) throw new Error('Content item could not be found');

    return response.data;
  }
}

export default new TargetService(axiosInstance, urlResource);
