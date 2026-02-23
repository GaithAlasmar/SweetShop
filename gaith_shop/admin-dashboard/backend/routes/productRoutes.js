const express = require('express');
const Product = require('../models/Product');
const { protect, admin } = require('../middleware/authMiddleware');
const router = express.Router();

// @desc Get all products
router.get('/', async (req, res) => {
    const products = await Product.find({});
    res.json(products);
});

// @desc Add a product (Admin only)
router.post('/', protect, admin, async (req, res) => {
    const { name, description, price, category, image, stock } = req.body;
    const product = new Product({ name, description, price, category, image, stock });
    const createdProduct = await product.save();
    res.status(201).json(createdProduct);
});

// @desc Update product (Admin only)
router.put('/:id', protect, admin, async (req, res) => {
    const product = await Product.findById(req.params.id);
    if (product) {
        product.name = req.body.name || product.name;
        product.price = req.body.price || product.price;
        // ... other fields
        const updatedProduct = await product.save();
        res.json(updatedProduct);
    } else {
        res.status(404).json({ message: 'Product not found' });
    }
});

// @desc Delete product (Admin only)
router.delete('/:id', protect, admin, async (req, res) => {
    const product = await Product.findById(req.params.id);
    if (product) {
        await product.deleteOne();
        res.json({ message: 'Product removed' });
    } else {
        res.status(404).json({ message: 'Product not found' });
    }
});

module.exports = router;
