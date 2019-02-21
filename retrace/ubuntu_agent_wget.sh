wget https://s1.stackify.com/Account/AgentDownload/Linux --output-document=stackify.tar.gz && \
tar -zxvf stackify.tar.gz stackify-agent-install-32bit && \
cd stackify-agent-install-32bit && \
sudo ./agent-install.sh --docker --key "YOUR ACTIVACTION KEY" --environment "YOUR ENVIRONMENT NAME"
