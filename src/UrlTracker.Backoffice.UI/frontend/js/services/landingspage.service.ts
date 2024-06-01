import { Axios } from 'axios';
import { axiosInstance } from '../util/tools/axios.service';
import urlresource, { IControllerUrlResource, IUrlResource } from '../util/tools/urlresource.service';

export interface ILandingspageService {
  numericMetric: () => Promise<number>;
}

export class LandingspageService implements ILandingspageService {
  constructor(
    private axios: Axios,
    private urlResource: IUrlResource,
  ) {}

  private get controller(): IControllerUrlResource {
    return this.urlResource.getController('LandingPage');
  }

  public async numericMetric() {
    const response = await this.axios.get<{
      value: number;
    }>(this.controller.getUrl('GetNumericMetric'));
    return response.data.value;
  }
}

export default new LandingspageService(axiosInstance, urlresource);
