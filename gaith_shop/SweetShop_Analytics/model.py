import numpy as np
import pandas as pd
from sklearn.linear_model import LinearRegression
from typing import List, Dict, Any

class SalesForecaster:
    def __init__(self):
        self.model = LinearRegression()

    def train_and_predict(self, data: List[Dict[str, Any]], days_to_predict: int = 7) -> List[Dict[str, Any]]:
        """
        Trains a simple linear regression model on the provided data and predicts future sales.
        
        Args:
            data: List of dictionaries containing 'date' and 'amount'.
            days_to_predict: Number of days to forecast.
            
        Returns:
            List of dictionaries containing 'date' and 'predicted_amount'.
        """
        if not data:
            return []

        df = pd.DataFrame(data)
        df['date'] = pd.to_datetime(df['date'])
        df = df.sort_values('date')
        
        # Create a day index (0, 1, 2, ...) for regression
        df['day_index'] = (df['date'] - df['date'].min()).dt.days
        
        X = df[['day_index']]
        y = df['amount']
        
        self.model.fit(X, y)
        
        last_day_index = df['day_index'].max()
        future_indices = np.array(range(last_day_index + 1, last_day_index + 1 + days_to_predict)).reshape(-1, 1)
        
        predictions = self.model.predict(future_indices)
        
        forecast = []
        start_date = df['date'].min()
        
        for i, pred in zip(future_indices.flatten(), predictions):
            future_date = start_date + pd.Timedelta(days=i)
            forecast.append({
                "date": future_date.strftime("%Y-%m-%d"),
                "predicted_amount": max(0, round(float(pred), 2)) # Ensure no negative sales
            })
            
        return forecast

class Recommender:
    def __init__(self):
        # In a real scenario, this would load a trained model or association rules
        pass

    def recommend(self, current_cart_product_ids: List[int]) -> List[int]:
        """
        Returns a list of recommended product IDs based on the current cart.
        
        Args:
            current_cart_product_ids: List of product IDs currently in the cart.
            
        Returns:
            List of recommended product IDs.
        """
        # specialized logic for demo purposes
        # If 'Lazy Cake' (assume ID 1) is in cart, recommend 'Coffee' (assume ID 5)
        
        recommendations = set()
        
        # Mock logic: Simple association rules
        # In a real app, these IDs would come from the database
        # For now, we'll return some dummy IDs likely to exist or be placeholders
        
        # Logic: Recommend 2 random items not in the cart (mock behavior)
        # Assuming product IDs range from 1 to 20
        candidates = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
        
        for pid in candidates:
            if pid not in current_cart_product_ids:
                recommendations.add(pid)
                if len(recommendations) >= 3:
                    break
        
        return list(recommendations)
