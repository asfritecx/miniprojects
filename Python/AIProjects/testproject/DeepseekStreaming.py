from openai import OpenAI
client = OpenAI(api_key="", base_url="https://api.deepseek.com")

# Round 1
messages = [{"role": "user", "content": "Write a simple api interface in asp.net core 8"}]
response = client.chat.completions.create(
    model="deepseek-reasoner",
    messages=messages,
    stream=True,
    temperature=0.0
)

reasoning_content = ""
content = ""

for chunk in response:
    if chunk.choices[0].delta.reasoning_content:
        reasoning_content += chunk.choices[0].delta.reasoning_content
    else:
        content += chunk.choices[0].delta.content

# Round 2
messages.append({"role": "assistant", "content": content})
messages.append({'role': 'user', 'content': "Create the client api request to interact with the api"})
response = client.chat.completions.create(
    model="deepseek-reasoner",
    messages=messages,
    stream=True,
    temperature=0.0
)