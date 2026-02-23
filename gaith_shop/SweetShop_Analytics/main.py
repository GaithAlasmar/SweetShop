from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from typing import List, Dict, Any
from model import SalesForecaster, Recommender
import uvicorn

app = FastAPI()

# Data Models
class SalesData(BaseModel):
    date: str
    amount: float

class PredictionRequest(BaseModel):
    historical_data: List[SalesData]
    days_to_predict: int = 7

class RecommendationRequest(BaseModel):
    cart_product_ids: List[int]

# Initialize models
forecaster = SalesForecaster()
recommender = Recommender()

@app.post("/predict-sales")
async def predict_sales(request: PredictionRequest):
    try:
        data = [item.dict() for item in request.historical_data]
        forecast = forecaster.train_and_predict(data, request.days_to_predict)
        return {"forecast": forecast}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.post("/recommend-products")
async def recommend_products(request: RecommendationRequest):
    try:
        recommendations = recommender.recommend(request.cart_product_ids)
        return {"recommendations": recommendations}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8000)
