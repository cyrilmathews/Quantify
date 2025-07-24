import { Client } from "../../clients/models/client.interface";

export interface Job {
  id: number;
  clientId: number;
  code: string;
  name: string;
  createdBy: number;
  createdOn: string; // DateTime as ISO string
  updatedBy?: number;
  updatedOn?: string; // DateTime as ISO string
  version?: string; // byte[] from API will be base64 string
  client?: Client;
}
