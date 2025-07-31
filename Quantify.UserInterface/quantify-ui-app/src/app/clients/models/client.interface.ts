export interface Client {
  id: number;
  code: string;
  name: string;
  sourceVersion: string; // byte[] from API will be base64 string
  createdOn: string; // DateTime as ISO string
}