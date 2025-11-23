'use client'

import React, { useEffect, useState } from 'react';
import {
    getInventoryDashboard,
    getInventoryList,
    InventoryItemDto,
    InventoryDashboardDto,
    ProductInventoryDto,
} from '../../../services/api/inventory.api';

const InventoryPage: React.FC = () => {
    const [dashboard, setDashboard] = useState<InventoryDashboardDto | null>(null);
    const [inventory, setInventory] = useState<ProductInventoryDto[]>([]);
    const [category, setCategory] = useState<string>("");
    const [location, setLocation] = useState<number | null>(null);
    useEffect(() => {
        loadDashboard();
        loadInventory();
    }, []);

    const loadDashboard = async () => {
        const data = await getInventoryDashboard();
        setDashboard(data);
    };

    const loadInventory = async () => {
        const data = await getInventoryList();
        setInventory(data);
    };

    return (
        <div className="p-6">
            <h1 className="text-2xl font-bold mb-6">📦 Inventory Dashboard</h1>

            {/* ========== Top Metrics ========== */}
            <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-8">
                <div className="p-4 bg-blue-100 rounded shadow">
                    <p className="text-gray-600">Total Stock Value</p>
                    <p className="text-xl font-semibold">
                        {dashboard?.totalStockValue?.toLocaleString()} ₫
                    </p>
                </div>

                <div className="p-4 bg-yellow-100 rounded shadow">
                    <p className="text-gray-600">Low Stock Items</p>
                    <p className="text-xl font-semibold">{dashboard?.lowStockCount}</p>
                </div>

                <div className="p-4 bg-red-100 rounded shadow">
                    <p className="text-gray-600">Out of Stock</p>
                    <p className="text-xl font-semibold">{dashboard?.outOfStockCount}</p>
                </div>

                <div className="p-4 bg-green-100 rounded shadow">
                    <p className="text-gray-600">Inventory Turnover Rate</p>
                    <p className="text-xl font-semibold">
                        {dashboard?.turnoverRate ?? "-"} / month
                    </p>
                </div>
            </div>

            {/* ========== Filters ========== */}
            <div className="flex gap-4 mb-6">
                <input
                    type="text"
                    placeholder="Filter by category..."
                    className="border px-3 py-2 rounded"
                    value={category}
                    onChange={(e) => setCategory(e.target.value)}
                />

                <input
                    type="number"
                    placeholder="Location ID"
                    className="border px-3 py-2 rounded"
                    onChange={(e) => setLocation(Number(e.target.value))}
                />

                <button
                    onClick={loadInventory}
                    className="px-4 py-2 bg-blue-500 text-white rounded"
                >
                    Apply Filters
                </button>
            </div>

            {/* ========== Inventory Table ========== */}
            <div className="bg-white rounded shadow p-4">
                <h2 className="text-xl font-semibold mb-4">
                    📋 Product Inventory List
                </h2>

                <table className="w-full border">
                    <thead className="bg-gray-100">
                        <tr>
                            <th className="border px-3 py-2">Product</th>
                            <th className="border px-3 py-2">Category</th>
                            <th className="border px-3 py-2">Total</th>
                            <th className="border px-3 py-2 text-green-700">Available</th>
                            <th className="border px-3 py-2 text-orange-700">Allocated</th>
                            <th className="border px-3 py-2 text-red-700">Damaged</th>
                            <th className="border px-3 py-2 text-blue-700">In Transit</th>
                        </tr>
                    </thead>

                    <tbody>
                        {inventory.map((item) => (
                            <tr key={item.productId}>
                                <td className="border px-3 py-2">{item.productName}</td>
                                <td className="border px-3 py-2">{item.category}</td>
                                <td className="border px-3 py-2 font-semibold">
                                    {item.totalQuantity}
                                </td>
                                <td className="border px-3 py-2 text-green-700 font-medium">
                                    {item.available}
                                </td>
                                <td className="border px-3 py-2 text-orange-700 font-medium">
                                    {item.allocated}
                                </td>
                                <td className="border px-3 py-2 text-red-700 font-medium">
                                    {item.damaged}
                                </td>
                                <td className="border px-3 py-2 text-blue-700 font-medium">
                                    {item.inTransit}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>

                {inventory.length === 0 && (
                    <p className="text-center text-gray-500 py-4">
                        No inventory data found.
                    </p>
                )}
            </div>
        </div>
    );
};

export default InventoryPage;
