export interface API {
  success:  boolean;
  packages: Package[];
}

export interface Package {
  type:              Type;
  name:              string;
  title:             string;
  author:            string;
  languages:         Language[];
  systems:           string[] | null;
  short_description: string;
  description:       string;
  url:               string;
  manifest:          string;
  tags:              { [key: string]: string };
  screenshots:       string[];
  price:             number;
  installs:          string;
  latest?:           string;
  update:            number;
  versions:          Version | string[];
}

export interface Version {
  [versionNum: string]: string;
}

export interface Language {
  name?: string;
  lang:  string;
}

export enum Type {
  Module = "module",
  System = "system",
  World = "world",
}
