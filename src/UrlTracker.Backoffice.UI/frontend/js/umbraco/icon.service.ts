export interface IIconHelper {

    getAllIcons(): angular.IPromise<IIcon[]>;
}

export interface IIcon {
    name: string;
    svgString: {$$unwrapTrustedValue: () => string};
}