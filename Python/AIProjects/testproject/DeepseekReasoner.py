from openai import OpenAI
client = OpenAI(api_key="", base_url="https://api.deepseek.com")

# Initialize conversation history
conversation_history = []

# Round 1
user_message = {"role": "user", "content": "Write a simple api interface in asp.net core 8"}
conversation_history.append(user_message)

response = client.chat.completions.create(
    model="deepseek-chat",
    messages=conversation_history
)

assistant_response = {
    "role": "assistant",
    "content": response.choices[0].message.content
}
conversation_history.append(assistant_response)

print("After Round 1:", conversation_history)

# Round 2
new_user_message = {"role": "user", "content": "Create the client api request to interact with the api"}
conversation_history.append(new_user_message)

response = client.chat.completions.create(
    model="deepseek-chat",
    messages=conversation_history  # Contains full history
)

assistant_response = {
    "role": "assistant",
    "content": response.choices[0].message.content
}
conversation_history.append(assistant_response)

print("After Round 2:", conversation_history)