#-*- coding: UTF-8 -*-

import socket
import threading
import sys
import json

reload(sys);
sys.setdefaultencoding('utf8');

SERVERIP='127.0.0.1';
SERVERPORT=9999;

LOGINFLAG=False; 

class User(object):
	nickname="";
	password="";
	colorId=0;
	def __init__(self,nickname,password,colorId):
		self.nickname=nickname;
		self.password=password;
		self.colorId=colorId;

def listenning(sock, addr):
	global LOGINFLAG;
	while True:	
		try:
			data = sock.recv(1024);
		except:
			sock.close()
			print("Failed to connect the server. You've been offline.")
			return 1;
		
		j=json.loads(data);	
		
		if (j["action"]=="send"):	
			if (j["id"]==-1):
				print j["content"].decode('utf-8').encode('gbk', 'ignore');
			else:
				print ("%d"%j["id"]+" "+j["senderName"]+":\t"+j["content"]).decode('utf-8').encode('gbk', 'ignore')
		elif (j["action"]=="get"):
			print ("%d"%j["id"]+" "+j["senderName"]+":\t"+j["content"]).decode('utf-8').encode('gbk', 'ignore')
			print j["sons"],
			print (" colorId=%d"%j["colorId"]+" hot=%d"%j["hot"])
		elif (j["action"]=="user"):
			if (j["user"]=="wrong"):
				LOGINFLAG=False; 
				print "Sorry, wrong passwords.";
			elif (j["user"]=="new"):
				LOGINFLAG=True; 
				print "The user has just been registered.";
			elif (j["user"]=="accept"):
				LOGINFLAG=True; 
				print "Log in successed.";
		
print "Input your name:     ",
userName=raw_input();
print "Input your password: ",
userPassword=raw_input();

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
addr=(SERVERIP, SERVERPORT)

try:
	s.connect(addr)
except:
	print("Server ERROR. Please check the server: %s:%s" % addr)
	sys.exit(1)

c=0; 
for i in userName: # such an easy way to calculate the colorId.
	c+=ord(i);	
c%=360;
user=User(userName,userPassword,c);

j0={"nickname":user.nickname,"password":user.password,"colorId":user.colorId}
s.send(json.dumps(j0));

t = threading.Thread(target=listenning, args=(s, addr))
t.start()

while True:
	action=raw_input();
	if (not LOGINFLAG):
		print "You haven't logged in."
		continue;
	if "s" in action:
		try:
			father=int(raw_input("input the ID to RETURN: "));
		except:
			father=-1;
		str=raw_input("input the message: ");
		j1={"action":"send","senderName":user.nickname,"colorId":user.colorId,"content":str,"father":father}		
		s.send(json.dumps(j1));
	else:
		try:
			father=int(raw_input("input the ID to CHECK: "));
		except:
			father=-1;
		j1={"action":"get","id":father}
		s.send(json.dumps(j1));
		
s.close();