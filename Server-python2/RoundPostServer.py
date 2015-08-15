#-*- coding: UTF-8 -*-

import socket
import threading
import json
import pickle
import random

import sys
reload(sys)
sys.setdefaultencoding('utf8')

messages=[];
userList=[];

class User(object):
	nickname="";
	password="";
	colorId=0;
	def __init__(self,nickname,password,colorId):
		self.nickname=nickname;
		self.password=password;
		self.colorId=colorId;
		
class Message(object):
	id=0;
	content="";
	father=0;
	sons=[];
	senderName="";
	colorId=0;
	hot=0;
	place=-1;
	def __init__(self,id,content,father,senderName,colorId):
		self.id=id;
		self.content=content;		
		if (father<-1 or father>=len(messages)):
			father=0;
		self.father=father;
		self.senderName=senderName;
		self.colorId=colorId;
		
		if (father<=0):
			place=-1;
		else:
			q=len(messages[father].sons);
			q=q-q%12;
			ll=range(q,q+12);
			for sonId in messages[father].sons:
				if (messages[sonId].place in ll):
					ll.remove(messages[sonId].place);
			self.place=random.choice(ll);
		
		self.sons=[];
		if (father!=-1): 
			messages[father].appendSon(id);
		
	def appendSon(self,sonId):
		self.sons.append(sonId);
	
	def addHot(self,i=1):
		self.hot+=i;
		
def addMessage(j):
	id=len(messages);
	messages.append(Message(id,j["content"],j["father"],j["senderName"],j["colorId"]))
	if j["father"]>0:
		messages[j["father"]].addHot(2);
		if messages[j["father"]].father>0:
			messages[messages[j["father"]].father].addHot(1);
	messageToAll(j["content"],j["senderName"],id)

def getMessage(id):
	if (id<0 or id>=len(messages)):
		id=0;
	m=messages[id];
	j0={"action":"get",
		"id":m.id,
		"senderName":m.senderName,
		"colorId":m.colorId,
		"content":m.content,
		"father":m.father,
		"sons":m.sons,
		"hot":m.hot,
		"place":m.place
		}
	return json.dumps(j0);

def saveMessage():
	print "Trying to save the messages... ",
	output = open('data.pkl', 'wb');
	pickle.dump(messages, output);
	output.close();
	print "The messages has been saved."
	
def allUserNames():
	str=""
	for (s,a,n) in userList:
		str+=n.nickname+" ";
	return "当前用户: "+str

def deleteUser(s,a,n):
	userList.remove((s,a,n))
	messageToAll("用户 %s 已下线." % n.nickname)
	messageToAll(allUserNames())
	print "Connection %s:%s closed." % a
	s.close()
	saveMessage();
	
def messageToAll(data,senderName="",id=-1):
	j0={"action":"send","content":data,"senderName":senderName,"id":id}
	str=json.dumps(j0);
	if (id==-1):
		print("\t\t"+data.decode('utf-8').encode('gbk', 'ignore')) # id==-1 means it's a system info.
	else:
		str=getMessage(id);
	for (s,a,n) in userList:
		try:
			s.send(str)			
		except:
			deleteUser(s,a,n)

def sendToAll(str):
	for (s,a,n) in userList:
		try:
			s.send(str)			
		except:
			deleteUser(s,a,n)

def tcplink(sock, addr):
	str=sock.recv(1024)
	j=json.loads(str)
	newUser=User(j["nickname"],j["password"],j["colorId"]);
	userList.append((sock,addr,newUser))
	
	userName=newUser.nickname;
	
	messageToAll("用户 "+userName+" 已进入.")
	messageToAll(allUserNames())
	
	print "New connection from %s:%s, %s" % (addr[0],addr[1],userName.decode('utf-8').encode('gbk', 'ignore')) 

	while True:
		try:
			data = sock.recv(1024)
		except:
			deleteUser(sock,addr,newUser)
			return 1
		
		j=json.loads(data)
		if (j["action"]=="send"):
			addMessage(j)
		elif (j["action"]=="get"):
			sock.send(getMessage(j["id"]))

try:
	print "Trying to load the messages... ",
	pkl_file = open('data.pkl', 'rb')
	messages = pickle.load(pkl_file)
	pkl_file.close();
	print "The messages has been loaded."
except:
	messages=[];
	messages.append(Message(0,"",-1,"root",240)); # root of the forest.
			
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind(("", 9999))
s.listen(5)
print("Waiting for connection...")
while True:
	sock, addr = s.accept()
	t = threading.Thread(target=tcplink, args=(sock, addr))
	t.start()