#!/bin/bash
die () {
    echo >&2 "$@"
    exit 1
}

[ "$#" -ge 2 ] || die "usage: init-letsencrypt.sh [staging | production] [email] <domain1 domain2 ...>"

domains=(${@:3})
rsa_key_size=4096
data_path="./nginx/$1/certbot"
email="$2" # Adding a valid address is strongly recommended
staging=0 # Set to 1 if you're testing your setup to avoid hitting request limits


if [ -d "$data_path/conf/live/" ]; then
  read -p "Existing data found. Continue and replace existing certificates? (y/N) " decision
  if [ "$decision" != "Y" ] && [ "$decision" != "y" ]; then
    exit
  fi
fi

if [ ! -e "$data_path/conf/options-ssl-nginx.conf" ] || [ ! -e "$data_path/conf/ssl-dhparams.pem" ]; then
  echo "### Downloading recommended TLS parameters ..."
  mkdir -p "$data_path/conf"
  # don't need the below because using mozilla's ssl config
  # curl -s https://raw.githubusercontent.com/certbot/certbot/master/certbot-nginx/certbot_nginx/options-ssl-nginx.conf > "$data_path/conf/options-ssl-nginx.conf"
  curl -s https://raw.githubusercontent.com/certbot/certbot/master/certbot/ssl-dhparams.pem > "$data_path/conf/ssl-dhparams.pem"
  echo
fi

for domain in "${domains[@]}"; do
  echo "### Removing old certificate for $domain ..."
  docker-compose -f docker-compose.$1.yml run --rm --entrypoint "\
    rm -Rf /etc/letsencrypt/live/$domain && \
    rm -Rf /etc/letsencrypt/archive/$domain && \
    rm -Rf /etc/letsencrypt/renewal/$domain.conf" certbot
  echo
done

for domain in "${domains[@]}"; do
  echo "### Creating dummy certificate for $domain ..."
  path="/etc/letsencrypt/live/$domain"
  mkdir -p "$data_path/conf/live/$domain"
  docker-compose -f docker-compose.$1.yml run --rm --entrypoint "\
    openssl req -x509 -nodes -newkey rsa:1024 -days 1\
      -keyout "$path/privkey.pem" \
      -out "$path/fullchain.pem" \
      -subj '/CN=localhost'" certbot
  echo
done

echo "### Starting nginx ..."
docker-compose -f docker-compose.$1.yml up --force-recreate -d
echo

for domain in "${domains[@]}"; do
  echo "### Removing dummy certificate for $domain ..."
  docker-compose -f docker-compose.$1.yml run --rm --entrypoint "\
    rm -Rf /etc/letsencrypt/live/$domain" certbot
  echo
done

echo "### Requesting Let's Encrypt certificates ..."

# Select appropriate email arg
case "$email" in
  "") email_arg="--register-unsafely-without-email" ;;
  *) email_arg="--email $email" ;;
esac

# Enable staging mode if needed
if [ $staging != "0" ]; then staging_arg="--staging"; fi

for domain in "${domains[@]}"; do
  docker-compose -f docker-compose.$1.yml run --rm --entrypoint "\
    certbot certonly --webroot -w /var/www/certbot \
      $staging_arg \
      $email_arg \
      -d $domain \
      --rsa-key-size $rsa_key_size \
      --agree-tos \
      --force-renewal" certbot
  echo
done

echo "### Reloading nginx ..."
docker-compose -f docker-compose.$1.yml exec proxy nginx -s reload