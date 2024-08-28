import { createContext } from '@lit/context';
import type { IOverlayService } from '@/umbraco/overlay.service';
export type { IOverlayService } from '@/umbraco/overlay.service';
export const overlayServiceContextKey = 'overlayServiceContext';
export const overlayServiceContext = createContext<IOverlayService>(overlayServiceContextKey);
