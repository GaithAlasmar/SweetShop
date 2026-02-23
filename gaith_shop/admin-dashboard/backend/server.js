const express = require('express');
const mongoose = require('mongoose');
const cors = require('cors');
require('dotenv').config();

const app = express();
const PORT = process.env.PORT || 5000;

// Middleware
app.use(cors());
app.use(express.json());

// Routes
app.use('/api/users', require('./routes/userRoutes'));
app.use('/api/products', require('./routes/productRoutes'));

// Basic Route
app.get('/', (req, res) => {
  res.send('Admin Dashboard Backend is running...');
});

// Database Connection
mongoose
  .connect(process.env.MONGO_URI || 'mongodb://localhost:27017/sweetshop_admin')
  .then(() => console.log('MongoDB Connected'))
  .catch((err) => console.log('DB Connection Error:', err));

app.listen(PORT, () => {
  console.log(`Server is running on port ${PORT}`);
});
