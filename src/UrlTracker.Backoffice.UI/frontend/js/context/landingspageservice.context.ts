import { ILandingspageService } from '@/services/landingspage.service';
import { createContext } from '@lit/context';
export type { ILandingspageService } from '../services/landingspage.service';
export const landingpageServiceKey = 'landingspageService';
export const landingpageServiceContext = createContext<ILandingspageService>(landingpageServiceKey);
