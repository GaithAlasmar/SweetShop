import requests
import json
import time
import sys

BASE_URL = "http://localhost:8000"

def test_predict_sales():
    print("Testing /predict-sales...")
    data = {
        "historical_data": [
            {"date": "2023-01-01", "amount": 100},
            {"date": "2023-01-02", "amount": 110},
            {"date": "2023-01-03", "amount": 105},
            {"date": "2023-01-04", "amount": 115},
            {"date": "2023-01-05", "amount": 120},
            {"date": "2023-01-06", "amount": 125},
            {"date": "2023-01-07", "amount": 130}
        ],
        "days_to_predict": 3
    }
    
    try:
        response = requests.post(f"{BASE_URL}/predict-sales", json=data)
        if response.status_code == 200:
            print("Success!")
            print(json.dumps(response.json(), indent=2))
        else:
            print(f"Failed with status code: {response.status_code}")
            print(response.text)
            sys.exit(1)
    except Exception as e:
        print(f"Error: {e}")
        sys.exit(1)

def test_recommend_products():
    print("\nTesting /recommend-products...")
    data = {
        "cart_product_ids": [1, 2]
    }
    
    try:
        response = requests.post(f"{BASE_URL}/recommend-products", json=data)
        if response.status_code == 200:
            print("Success!")
            print(json.dumps(response.json(), indent=2))
        else:
            print(f"Failed with status code: {response.status_code}")
            print(response.text)
            sys.exit(1)
    except Exception as e:
        print(f"Error: {e}")
        sys.exit(1)

if __name__ == "__main__":
    # Wait for server to start - handled by caller script now
    test_predict_sales()
    test_recommend_products()
