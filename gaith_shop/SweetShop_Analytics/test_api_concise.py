import requests
import sys
import time

BASE_URL = "http://localhost:8000"

def test():
    print("Starting tests...")
    
    # Predict Sales
    try:
        data = {
            "historical_data": [{"date": "2023-01-01", "amount": 100}],
            "days_to_predict": 1
        }
        resp = requests.post(f"{BASE_URL}/predict-sales", json=data, timeout=5)
        if resp.status_code == 200:
            print("Predict Sales: OK")
        else:
            print(f"Predict Sales: FAILED {resp.status_code}")
            sys.exit(1)
    except Exception as e:
        print(f"Predict Sales: ERROR {e}")
        sys.exit(1)

    # Recommend Products
    try:
        data = {"cart_product_ids": [1]}
        resp = requests.post(f"{BASE_URL}/recommend-products", json=data, timeout=5)
        if resp.status_code == 200:
            print("Recommend Products: OK")
        else:
            print(f"Recommend Products: FAILED {resp.status_code}")
            sys.exit(1)
    except Exception as e:
        print(f"Recommend Products: ERROR {e}")
        sys.exit(1)

    print("All tests passed.")

if __name__ == "__main__":
    test()
