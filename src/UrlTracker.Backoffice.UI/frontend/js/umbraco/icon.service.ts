export interface IIconHelper {
  getAllIcons(): angular.IPromise<IIcon[]>;
  getIcon(iconName: string): angular.IPromise<IIcon | undefined>;
}

export interface IIcon {
  name: string;
  svgString: { $$unwrapTrustedValue: () => string };
}
