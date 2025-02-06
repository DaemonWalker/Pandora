import React, { useState } from 'react';
import { Select, Input, Button, List, Pagination, Row, Col } from 'antd';
import { useSearchStore } from '../store/useSearchStore';

const contentTypes = [{ value: 0, label: "全部" }, { value: 1, label: "电影" }, { value: 2, label: "电视剧" }];

const { Item } = List;


export const Search: React.FC = () => {
    const { searchResults, error, isLoading, search } = useSearchStore();
    const [selectedTypes, setSelectedTypes] = useState(0);
    const [searchText, setSearchText] = useState('');
    const [currentPage, setCurrentPage] = useState(1);
    const [pageSize, setPageSize] = useState(10);


    const handleTypeChange = (value: number) => {
        setSelectedTypes(value);
    };

    const handleSearchTextChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setSearchText(e.target.value);
    };

    const handleSearch = async () => {
        await search(searchText);
    };

    const handlePageChange = (page: number, pageSize: number) => {
        setCurrentPage(page);
        setPageSize(pageSize);
    };

    // 模拟搜索函数
    const performSearch = (type: number, text: string) => {
        // 这里可以根据实际情况进行搜索逻辑的实现
        // 假设返回一个包含搜索结果的数组
        return Array.from({ length: 50 }, (_, i) => `Result ${i + 1}`);
    };

    // 计算当前页的搜索结果
    const currentPageResults = searchResults.slice((currentPage - 1) * pageSize, currentPage * pageSize);

    return (
        <>
            <Row>
                <Select
                    placeholder="类型"
                    value={selectedTypes}
                    onChange={handleTypeChange}
                    style={{ width: '200px', marginRight: '10px' }}
                    options={contentTypes}
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
            </Row>
            <Row justify="center">
                <List loading={isLoading}
                    itemLayout="horizontal"
                    dataSource={currentPageResults}
                    renderItem={(item, index) => (
                        <Item key={index}>
                            {item}
                        </Item>
                    )}
                />
            </Row>
            <Row justify="center">
                <Pagination
                    current={currentPage}
                    pageSize={pageSize}
                    total={searchResults.length}
                    onChange={handlePageChange}
                />
            </Row>
        </>
    );
};