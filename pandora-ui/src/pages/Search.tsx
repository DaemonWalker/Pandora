import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { Select, Input, Button, List, Row, Col, message } from 'antd';
import { useSearchStore } from '../store/useSearchStore';
import { InfoModel } from '../models/InfoModel';
import { LinkType } from '../models/LinkType';
import { useAutoFetchTabKeys } from '../store/allSourceStore';

const { Item } = List;
const { Meta } = Item;


export const Search: React.FC = () => {
    const { searchResults, isLoading, search, error } = useSearchStore();
    const [selectedTypes, setSelectedTypes] = useState('');
    const [searchText, setSearchText] = useState('');

    const { tabKeys, isLoading: isSourceLoading } = useAutoFetchTabKeys();

    const sourceOptions = useMemo(() => {
        if (!tabKeys) {
            return []
        }
        return tabKeys.map(s => ({ value: s, label: s }))
    }, [tabKeys])
    useEffect(() => {
        if (tabKeys && tabKeys.length > 0) {
            setSelectedTypes(tabKeys[0]);
        }
    }, [tabKeys])
    useEffect(() => {
        if (!isLoading && error) {
            message.error(error)
        }
    }, [isLoading, error])

    const handleTypeChange = (value: string) => {
        setSelectedTypes(value);
    };

    const handleSearchTextChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setSearchText(e.target.value);
    };

    const handleSearch = async () => {
        await search(selectedTypes, searchText);
        console.log(error)
    };

    const handleGetMagnetLink = async (source: string, item: InfoModel) => {
        let magnetLink = "";
        let ok = true;
        if (item.linkType === LinkType.Magnet) {
            magnetLink = item.link;
        } else {
            try {
                const response = await fetch(`/api/Search/GetMagnet/${source}`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(item.link)
                });
                if (response.ok) {
                    magnetLink = await response.text();
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
        <div style={{ display: "flex", flexDirection: "column", flex: 1, maxHeight: "100%", gap: "10px" }}>
            <Row justify="center" gutter={[10, 10]}>
                <Col lg={4}>
                    <Select
                        placeholder="类型"
                        value={selectedTypes}
                        onChange={handleTypeChange}
                        style={{ width: '100%', marginRight: '10px' }}
                        options={sourceOptions}
                        loading={isSourceLoading}
                    />
                </Col>
                <Col lg={8}>
                    <Input
                        placeholder="Enter search text"
                        value={searchText}
                        onChange={handleSearchTextChange}
                    // style={{ width: '300px', marginRight: '10px' }}
                    />
                </Col>
                <Col lg={3}>
                    <Button type="primary" onClick={handleSearch}>
                        搜索
                    </Button>
                </Col>
            </Row>
            <Row style={{ flex: 1, overflowX: "auto" }}>
                <Col span={24}>
                    <List
                        loading={isLoading}
                        itemLayout="horizontal"
                        dataSource={searchResults}
                        renderItem={(source, index) =>
                            <List
                                key={`${source}${index}`}
                                itemLayout="horizontal"
                                dataSource={source.infos}
                                renderItem={(item, idx) =>
                                    <Item key={`${source}${index}${idx}`}
                                        actions={[
                                            <Button onClick={() => handleGetMagnetLink(source.source, item)} type='link'>
                                                获取磁力链接
                                            </Button>]}>
                                        <Meta title={item.name} description={item.size}>
                                            {/* {item.name} */}
                                        </Meta>
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
        </div>
    );
};