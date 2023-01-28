FROM mcr.microsoft.com/dotnet/sdk:6.0
RUN dotnet tool install --global dotnet-ef --version 6.*
RUN dotnet dev-certs https
ENV PATH $PATH:/root/.dotnet/tools
RUN git config --global user.email "kamilinho20@gmail.com"
RUN git config --global user.name "Kamil Stolarczyk"