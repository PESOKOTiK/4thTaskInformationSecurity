# Clone the repository
git clone https://github.com/yourusername/rsa-digital-signature-level3.git
cd rsa-digital-signature-level3

# In three separate terminals, run:
# Terminal 1: Signer
cd Signer
dotnet run

# Terminal 2: Tamper
cd ../Tamper
dotnet run

# Terminal 3: Verifier
cd ../Verifier
dotnet run
