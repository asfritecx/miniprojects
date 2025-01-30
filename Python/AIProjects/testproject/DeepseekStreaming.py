from openai import OpenAI
client = OpenAI(api_key="", base_url="https://api.deepseek.com")

# Get user input
user_prompt = input("What would you like to ask? ")

# Round 1
messages = [{"role": "user", "content": user_prompt}]
response = client.chat.completions.create(
    model="deepseek-reasoner",
    messages=messages,
    stream=True,
    temperature=0.6
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
user_followup = input("Any follow-up question? ")
messages.append({'role': 'user', 'content': user_followup})
response = client.chat.completions.create(
    model="deepseek-reasoner",
    messages=messages,
    stream=True,
    temperature=0.6
)