import React, { useMemo, useState } from 'react';
import { Select, Input, Button, List, Pagination, Row, Col, message } from 'antd';
import { useSearchStore } from '../store/useSearchStore';
import { InfoModel } from '../models/InfoModel';
import { LinkType } from '../models/LinkType';
import { useAutoFetchTabKeys } from '../store/allSourceStore';
import { ALL } from '../models/Constants';

const { Item } = List;


export const Search: React.FC = () => {
    const { tabKeys, isLoading: isSourceLoading } = useAutoFetchTabKeys();
    const { searchResults, isLoading, search } = useSearchStore();
    const [selectedTypes, setSelectedTypes] = useState(ALL);
    const [searchText, setSearchText] = useState('');
    const [currentPage, setCurrentPage] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const sourceOptions = useMemo(() => {
        if (!tabKeys) {
            return []
        }
        return [ALL, ...tabKeys].map(s => ({ value: s, label: s }))
    }, [tabKeys])


    const handleTypeChange = (value: string) => {
        setSelectedTypes(value);
    };

    const handleSearchTextChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setSearchText(e.target.value);
    };

    const handleSearch = async () => {
        await search(selectedTypes, searchText);
    };

    const handlePageChange = (page: number, pageSize: number) => {
        setCurrentPage(page);
        setPageSize(pageSize);
    };

    // 计算当前页的搜索结果
    const currentPageResults = searchResults.slice((currentPage - 1) * pageSize, currentPage * pageSize);

    const handleGetMagnetLink = async (source: string, item: InfoModel) => {
        let magnetLink = "";
        let ok = true;
        if (item.linkType === LinkType.Magnet) {
            magnetLink = item.link;
        } else {
            try {
                const response = await fetch(`/api/Search/GetMagnetLink/${source}`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(item.link)
                });
                if (response.ok) {
                    magnetLink = JSON.stringify(response);
                } else {
                    ok = false;
                }
            } catch (error) {
                ok = false;
            }
        }
        if (ok) {
            navigator.clipboard.writeText(magnetLink);
            message.success('复制成功');
        } else {
            message.error('复制失败');
        }
    };

    return (
        <Row justify="center">
            <Col span={24}>
                <Select
                    placeholder="类型"
                    value={selectedTypes}
                    onChange={handleTypeChange}
                    style={{ width: '200px', marginRight: '10px' }}
                    options={sourceOptions}
                    loading={isSourceLoading}
                />
                <Input
                    placeholder="Enter search text"
                    value={searchText}
                    onChange={handleSearchTextChange}
                    style={{ width: '300px', marginRight: '10px' }}
                />
                <Button type="primary" onClick={handleSearch}>
                    搜索
                </Button>
            </Col>
            <Col span={20}>
                <List
                    loading={isLoading}
                    itemLayout="horizontal"
                    dataSource={currentPageResults}
                    renderItem={(source, index) =>
                        <List
                            key={`${source}${index}`}
                            itemLayout="horizontal"
                            dataSource={source.infos}
                            renderItem={(item, idx) =>
                                <Item key={`${source}${index}${idx}`}>
                                    <Col span={19}>
                                        <span>{item.name}</span>
                                    </Col>
                                    <Col span={2}>
                                        <span style={{ marginLeft: '10px' }}>{item.size}</span>
                                    </Col>
                                    <Col span={3} style={{ textAlign: 'right' }}>
                                        <Button onClick={() => handleGetMagnetLink(source.source, item)} type='link'>
                                            获取磁力链接
                                        </Button>
                                    </Col>
                                </Item>}
                        />}
                />
            </Col>
            {/* <Col span={24} >
                <Pagination
                    current={currentPage}
                    pageSize={pageSize}
                    total={searchResults.length}
                    onChange={handlePageChange}
                />
            </Col> */}
        </Row>
    );
};