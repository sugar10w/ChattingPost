# ChattingPost
类似贴吧+QQ群。是一个面向话题的交流服务。目标是将话题的讨论变得更方便、有趣，逻辑清晰。

- 用树形结构表现回贴内容，不强调时间顺序。
- 被其他人关注的回复(包括其父节点)都会增加关注值，因而变得更显眼。
- 在一个平台内可以同时进行多个话题的讨论，话题之间不会干扰。
- 一个人可以新建一个话题，或者选择一个父节点进行回复。
- 除了帖子信息外，可以查看临时的系统公告。

### 计划中的功能：
- 信息推送，快速了解当前热点内容
- 主页信息强化，提供更多信息
- 对内容进行一定的排序
- 允许用户开辟多块展板，限制访问人员
- 自由查看在线人员。
- 使用赞和踩，影响这个回复的关注值
- 添加话题管理员，可以自由删除回答，删除回答分支，修改父子节点关系
- 投票系统
- 设计更强大的数据结构，或者有效利用数据库工具

## 文件说明
###C# WPF 客户端
- ClientSocket.cs 负责与服务器端通讯
- MessagesKeeper.cs 是ClientSocket和Window1之间的信息中介，存储所有Message
- Window1.xaml.cs 聊天页面UI逻辑
- MainWindow.xaml.cs 登录页面
- InfoBox.cs 存放系统信息，也是ClientSocket和Window1之间的信息中介
- Message.cs 每一条帖子
- User.cs 用户信息
- MyColor.cs 管理颜色

### Python 服务器端
- RoundPostServer.py 主文件，python2.7
- user.pkl 存储用户名和密码
- data.pkl 存储所有Messages

### Python 客户端
- RoundPostClient.py 主文件，python2.7

## 类说明
使用python写服务器端，C#WPF写客户端

### 用户 User：
- 昵称 nickname
- 密码 password
- 颜色 color
- //编号 id
- //最后一次登录时间 lastLoginTime

### 信息 Message：
- 发送用户名称 senderName
- 颜色 color （与发送用户的颜色相同）
- 内容 content
- 父节点编号 father
- 子节点编号数组 sons
- 编号 id
- 关注度 hot
- //发送用户 sender
- //相对于父节点的角度位置 place （先用一个数字大概表示）
- //发送用户编号 senderId
- //信息发送时间 time 

## 用户 -> 服务器 "action"
- user 发送本用户信息
- get 请求指定ID的Mesaage
- send 发送跟帖

## 服务器 -> 用户 "action"
- user 发送用户登录情况信息
- get 发送指定ID的Message
- send 发送系统信息