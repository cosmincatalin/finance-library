FROM mcr.microsoft.com/devcontainers/dotnet:7.0

RUN sudo apt update
RUN sudo apt remove -y cmake
RUN sudo apt install -y \
    python3-pip \
    uncrustify \
    nodejs
RUN pip install \
    pre-commit==2.20.0
RUN git config --global --add safe.directory /workspace