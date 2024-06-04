# Author: Sakthi Santhosh
# Created on: 01/05/2024
#
# Custom Payment Gateway Simulator
from json import dumps
from requests import post

PG_JWT = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJzdXNwYXlnYXRld2F5IiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiUGF5bWVudEdhdGV3YXkiLCJleHAiOjE3MTk3Nzg1NjUsImlzcyI6InRlc3QuYXBpLnNha3RoaXNhbnRob3NoLmluIiwiYXVkIjoiZGFzaGJvYXJkLnNha3RoaXNhbnRob3NoLmluIn0.m9mOpogVdWexX7OsNJ4fkTjup-htvC76tUzoQkdE_7Y" # XXX: Received from Server
PAYMENT_SUCCESS = True
HEADERS = {
    "Content-Type": "application/json",
    "Authorization": f"Bearer {PG_JWT}"
}

def simulate_payment(client_token: str):
    if not PAYMENT_SUCCESS:
        return

    BODY = {
        "clientToken": client_token,
    }

    with post("http://localhost:5064/api/ticket/generate",
        json=BODY,
        headers=HEADERS,
        data=dumps(BODY)
    ) as response:
        if response.status_code != 200:
            print(response.text)
            print("Payment failed because your back-end failed to respond.")

    print("Ticket booked successfully.")


if __name__ == "__main__":
    simulate_payment("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZW1wdXNlci0wNSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkNsaWVudENoYWxsZW5nZSIsInBheWVyLWlkIjoiNSIsInBheW1lbnQtaWQiOiI0Iiwic2VhdHMtYm9va2VkIjoiRTEiLCJzY3JlZW4taWQiOiIyIiwibW92aWVzaG93LWlkIjoiMSIsImV4cCI6MTcxNzE1OTc3MywiaXNzIjoidGVzdC5hcGkuc2FrdGhpc2FudGhvc2guaW4iLCJhdWQiOiJkYXNoYm9hcmQuc2FrdGhpc2FudGhvc2guaW4ifQ.X2OCaJ1ewxMfYd3XFfB6C1RZ6K--YOABWzauYC7hNkw")
