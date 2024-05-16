export interface IUmbracoNotificationsService {
    success: (title: string, description: string) => void;
    error: (title: string, description: string) => void;
}