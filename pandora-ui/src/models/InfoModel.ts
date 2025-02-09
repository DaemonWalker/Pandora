import { LinkType } from "./LinkType"

export interface InfoModel {
    name: string
    link: string
    size?: string
    linkType: LinkType
    pageSize?: number
}