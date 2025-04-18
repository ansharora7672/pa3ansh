import requests
import random
import sys

API_BASE = "http://localhost:5191/api/quotes"
QUOTES_FILE = "quotes.txt"

def view_quotes():
    resp = requests.get(API_BASE)
    if resp.status_code != 200:
        print("Error fetching quotes")
        return
    for q in resp.json():
        author = q.get("author") or "Unknown"
        print(f"{q['id']}: \"{q['content']}\" — {author} [Likes: {q['likes']}]")

def add_quote():
    content = input("Quote: ").strip()
    if not content:
        print("Content required")
        return
    author = input("Author (optional): ").strip()
    resp = requests.post(API_BASE, json={"content": content, "author": author})
    print("Added" if resp.status_code == 201 else f"Error {resp.status_code}")

def show_random():
    resp = requests.get(API_BASE)
    if resp.status_code != 200:
        print("Error fetching quotes")
        return
    quotes = resp.json()
    if not quotes:
        print("No quotes available")
        return
    q = random.choice(quotes)
    author = q.get("author") or "Unknown"
    print(f"\"{q['content']}\" — {author} [Likes: {q['likes']}]")

def load_quotes():
    try:
        with open(QUOTES_FILE, 'r', encoding='utf-8') as f:
            lines = [l.strip() for l in f if l.strip()]
    except FileNotFoundError:
        print(f"File not found: {QUOTES_FILE}")
        return
    for line in lines:
        if " - " in line:
            content, author = line.split(" - ", 1)
        else:
            content, author = line, ""
        resp = requests.post(API_BASE, json={"content": content, "author": author})
        print("Loaded" if resp.status_code == 201 else "Error")

def main():
    while True:
        print("""
1) View all quotes
2) Add a new quote
3) Show a random quote
4) Load quotes from quotes.txt
5) Exit
""")
        choice = input("Select 1-5: ").strip()
        if choice == "1":
            view_quotes()
        elif choice == "2":
            add_quote()
        elif choice == "3":
            show_random()
        elif choice == "4":
            load_quotes()
        elif choice == "5":
            sys.exit(0)
        else:
            print("Invalid choice, enter 1‑5")

if __name__ == "__main__":
    main()
