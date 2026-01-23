export interface IIconHelper {
  getAllIcons(): Promise<IIcon[]>;
  getIcon(iconName: string): Promise<IIcon | undefined>;
}

export interface IIcon {
  name: string;
  svgString: { $$unwrapTrustedValue: () => string };
}
